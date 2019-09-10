using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartRetail.App.DAL.Helpers
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Tree<T>
    {
        [JsonProperty]
        public T Value;

        [JsonProperty("nodes")]
        public List<Tree<T>> Children;

        public Tree<T> Parent;

        public Tree()
        {
            Children = new List<Tree<T>>();
        }

        public Tree<T> AddChild(Tree<T> node)
        {
            node.Parent = this;
            Children.Add(node);
            return this;
        }

        public Tree<T> AddChild(T value)
        {
            var node = new Tree<T>
            {
                Value = value,
                Parent = this,
            };

            Children.Add(node);

            return node;
        }

        public bool IsRoot()
        {
            return Parent == null;
        }
        
        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() != GetType())
                return false;

            var node = (Tree<T>)obj;
            return node != null && Value.Equals(node.Value);
        }

        public static Tree<T> Search(Tree<T> tree, T value)
        {
            Tree<T> result = null;

            if (tree.Value.Equals(value)) return tree;
            else
            {
                if (tree.Children.Count > 0)
                {
                    foreach (var node in tree.Children)
                    {
                        result = Search(node, value);
                        if (result != null) break;
                    }
                }
            }

            return result;
        }


        public static List<T> ToList(Tree<T> root)
        {
            if (root == null)
                return null;

            var treeCollection = new List<T>();
            treeCollection.Add(root.Value);
            if (root.Children.Count > 0)
            {
                foreach (var node in root.Children)
                {
                    treeCollection.AddRange(ToList(node));
                }
            }
            return treeCollection;
        }
    }
}
