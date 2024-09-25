using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CompresionAritmetica
{
    public class ArithmeticCompression
    {
        private readonly Dictionary<char, (ushort Low, ushort High)> _probabilities;
        private readonly string _source;
        private const ushort defaultLow = 0;
        private const ushort defaultHigh = 0xFFFF;
        private const ushort MSD = 0x8000;
        private const ushort SSD = 0x4000;

        public ArithmeticCompression(string source)
        {
            _source = source;
            _probabilities = new Dictionary<char, (ushort Low, ushort High)>();
            CalculateProbabilities();
        }

        private void CalculateProbabilities()
        {
            Dictionary<char, ushort> frequencies = new Dictionary<char, ushort>();
            foreach (char symbol in _source)
            {
                if (!frequencies.ContainsKey(symbol))
                    frequencies[symbol] = 0;

                frequencies[symbol]++;
            }

            ushort total = (ushort)_source.Length;
            ushort low = 0;
            foreach (var pair in frequencies.OrderBy(p => p.Key))
            {
                ushort range = (ushort)((pair.Value * (long)defaultHigh) / total);
                ushort high = (ushort)(low + range);
                _probabilities[pair.Key] = (low, (ushort)(high - 1));
                low = high;
            }
        }

        public long Compress()
        {
            ushort low = defaultLow;
            ushort high = defaultHigh;
            ushort underflow = 0;

            using (MemoryStream output = new MemoryStream())
            {
                foreach (char symbol in _source)
                {
                    long range = (long)(high - low) + 1;
                    high = (ushort)(low + (range * _probabilities[symbol].High / defaultHigh) - 1);
                    low = (ushort)(low + (range * _probabilities[symbol].Low / defaultHigh));

                    while ((high & MSD) == (low & MSD))
                    {
                        output.WriteByte((byte)((low & MSD) >> 15));
                        low <<= 1;
                        high = (ushort)((high << 1) + 1);

                        
                        while (underflow > 0)
                        {
                            output.WriteByte((byte)(((low & MSD) >> 15) ^ 1));
                            underflow--;
                        }
                    }

                    while ((low & ~high & SSD) != 0)
                    {
                        underflow++;
                        low = (ushort)(low << 1);
                        high = (ushort)((high << 1) + 1);
                    }
                }

                output.WriteByte((byte)((low & MSD) >> 15));
                return output.Length;
            }
        }

        public double GetCompressedSize()
        {
            if (_source.Length == 0 || _probabilities.Count == 0)
                return 0;

            double sizeInBits = 0;
            foreach (char c in _source)
            {
                if (_probabilities.ContainsKey(c))
                {
                    var range = _probabilities[c];
                    double probability = (range.High - range.Low + 1) / (double)(defaultHigh + 1);
                    sizeInBits -= Math.Log(probability, 2); 
                }
            }

            return Math.Ceiling(sizeInBits / 8.0); 
        }
    }
}




