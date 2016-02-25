using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using GraphLight.Parser;

namespace GraphLight.Graph
{
    public class DrawingGraph : Graph<VertexAttrs, EdgeAttrs>, INotifyPropertyChanged
    {
        private string _label;

        public DrawingGraph(string label)
        {
            Label = label;
        }

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                RaisePropertyChanged("Label");
            }
        }

        public override double Width
        {
            get { return base.Width; }
            set
            {
                base.Width = value;
                RaisePropertyChanged("Width");
            }
        }

        public override double Height
        {
            get { return base.Height; }
            set
            {
                base.Height = value;
                RaisePropertyChanged("Height");
            }
        }

        #region Файловые операции

        /// <summary>
        ///   Читает граф из файла.
        /// </summary>
        /// <param name = "fileStream">Поток, из которого читать данные</param>
        /// <returns>Считанный граф</returns>
        public static DrawingGraph ReadFromFile(Stream fileStream)
        {
            var ws = new StringWriter();
            var scanner = new MyScanner(fileStream);
            var parser = new MyParser(scanner) {errors = {errorStream = ws}};
            parser.Parse();
            if (parser.errors.count > 0)
                throw new Exception(ws.ToString());
            return parser.ParsedGraph;
        }

        public void WriteToFile(Stream fileStream)
        {
            using (var sw = new StreamWriter(fileStream))
                WriteToFile(sw);
        }

        public void WriteToFile(StreamWriter sw)
        {
            sw.WriteLine("digraph {0}", Label);
            sw.WriteLine("{");

            sw.WriteLine("\t nodes:");
            foreach (var vertex in Verteces)
                sw.WriteLine("\t\t{0} [label=\"{1}\" rank={2} order={3}];",
                             vertex.Data.Id, vertex.Data.Label, vertex.Data.Rank, vertex.Data.Position);

            sw.WriteLine("\t edges:");
            foreach (DrawingEdge myEdge in Edges)
                sw.WriteLine("\t\t{0} -> {1} [color={2} thickness={3}]",
                             myEdge.Src.Data.Id, myEdge.Dst.Data.Id, myEdge.Color, myEdge.Thickness);

            sw.WriteLine("}");
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        protected override ICollection<T> CreateCollection<T>()
        {
            return new ObservableCollection<T>();
        }

        protected override IEdge<VertexAttrs, EdgeAttrs> CreateEdge(EdgeAttrs data)
        {
            return new DrawingEdge(data);
        }

        protected override IVertex<VertexAttrs, EdgeAttrs> CreateVertex(VertexAttrs data)
        {
            return new DrawingVertex(data);
        }
    }
}