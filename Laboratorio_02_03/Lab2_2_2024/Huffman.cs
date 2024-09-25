using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class HuffmanNode
{
    public char Character { get; set; }
    public int Frequency { get; set; }
    public HuffmanNode Left { get; set; }
    public HuffmanNode Right { get; set; }
}

public class HuffmanEncoding
{
    public HuffmanNode BuildHuffmanTree(string input)
    {
        
        Dictionary<char, int> frequencies = new Dictionary<char, int>();
        foreach (char c in input)
        {
            if (frequencies.ContainsKey(c))
            {
                frequencies[c]++;
            }
            else
            {
                frequencies[c] = 1;
            }
        }

        
        var nodes = new List<HuffmanNode>();
        foreach (var kvp in frequencies)
        {
            nodes.Add(new HuffmanNode { Character = kvp.Key, Frequency = kvp.Value });
        }

        
        while (nodes.Count > 1)
        {
            nodes = nodes.OrderBy(n => n.Frequency).ToList();

            var left = nodes[0];
            var right = nodes[1];

            var parent = new HuffmanNode
            {
                Character = '\0', 
                Frequency = left.Frequency + right.Frequency,
                Left = left,
                Right = right
            };

            nodes.Remove(left);
            nodes.Remove(right);
            nodes.Add(parent);
        }

        return nodes.FirstOrDefault();
    }

    public Dictionary<char, string> BuildHuffmanTable(HuffmanNode root)
    {
        var huffmanTable = new Dictionary<char, string>();
        BuildHuffmanTableRecursive(root, "", huffmanTable);
        return huffmanTable;
    }

    private void BuildHuffmanTableRecursive(HuffmanNode node, string code, Dictionary<char, string> table)
    {
        if (node == null)
            return;

        if (node.Left == null && node.Right == null)
        {
            table[node.Character] = code;
        }

        BuildHuffmanTableRecursive(node.Left, code + "0", table);
        BuildHuffmanTableRecursive(node.Right, code + "1", table);
    }

    public int GetHuffmanEncodedSize(string input)
    {
        var root = BuildHuffmanTree(input);
        var table = BuildHuffmanTable(root);
        int sizeInBits = 0;

        foreach (var pair in table)
        {
            
            sizeInBits += pair.Value.Length * input.Count(c => c == pair.Key);
        }

        return sizeInBits; 
    }


}


