using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmartRetail.App.DAL.BLL.DataStructures
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CathegoryTree<T>
    {
        [JsonProperty]
        public T Value;

        [JsonProperty]
        public List<CathegoryTree<T>> Nodes;

        public CathegoryTree<T> Parent;

        public CathegoryTree()
        {
            Nodes = new List<CathegoryTree<T>>();
        }
        
        public CathegoryTree<T> AddNode(CathegoryTree<T> node)
        {
            node.Parent = this;
            Nodes.Add(node);
            return this;
        }

        public CathegoryTree<T> AddNode(T value)
        {
            var node = new CathegoryTree<T>
            {
                Value = value,
                Parent = this,
            };
            
            Nodes.Add(node);

            return node;
        }

        public static CathegoryTree<T> CreateTree(T value, List<CathegoryTree<T>> nodes = null)
        {
            return new CathegoryTree<T> {Value = value, Nodes = nodes};
        }

        public static CathegoryTree<T> Search(CathegoryTree<T> tree, T value)
        {
            CathegoryTree<T> result = null;

            if (tree.Value.Equals(value)) return tree;
            else
            {
                if (tree.Nodes.Count > 0)
                {
                    foreach (var node in tree.Nodes)
                    {
                        result = Search(node, value);
                        if (result != null) break;
                    }
                }
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() != GetType()) 
                return false;
             
            var node = (CathegoryTree<T>)obj;
            return node != null && Value.Equals(node.Value);
        }

        public bool IsRoot()
        {
            return Parent == null;
        }

        public static void RemoveChild(CathegoryTree<T> node)
        {
            var parent = node.Parent;
            
            if (node.Nodes.Count > 0)
                parent.Nodes.AddRange(node.Nodes);
            
            parent.Nodes.Remove(node);
        }

        public static List<T> ToList(CathegoryTree<T> root)
        {
            if (root == null)
                return null;
            
            var treeCollection = new List<T>();
            treeCollection.Add(root.Value);
            if (root.Nodes.Count > 0)
            {
                foreach (var node in root.Nodes)
                {
                    treeCollection.AddRange(ToList(node));
                }    
            }

            return treeCollection;
        }

    }
}
