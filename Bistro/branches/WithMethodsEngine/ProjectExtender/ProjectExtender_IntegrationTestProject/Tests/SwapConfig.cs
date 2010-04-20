using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSharp.ProjectExtender;
namespace IntegrationTests
{
    internal struct MoveOp
    {
        public int Index { get; set; }
        public CompileOrderViewer.Direction Dir { get; set; }
    }
    internal interface ISwapConfig
    {
        ISwapConfig SetOrder(params string[] filenames);
        ISwapConfig SetMoves(params MoveOp[] moves);
        List<string> FileOrder { get; }
        List<MoveOp> Moves { get; }
        string ConfigName { get;}
    }
    internal class SwapConfig : ISwapConfig
    {
        List<string> fileList = new List<string>();
        List<MoveOp> actions = new List<MoveOp>();
        string name;
        internal SwapConfig(string configName)
        {
            this.name = configName;
        }
        public ISwapConfig SetOrder(params string[] filenames)
        {
            foreach (string item in filenames)
                fileList.Add(item);
            return this;
        }
        public ISwapConfig SetMoves(params MoveOp[] moves)
        {
            foreach (MoveOp move in moves)
                actions.Add(move);
            return this;

        }
        public List<string> FileOrder { get { return fileList; } }
        public List<MoveOp> Moves { get { return actions; } }
        public string ConfigName { get { return name; } }
    }
}
