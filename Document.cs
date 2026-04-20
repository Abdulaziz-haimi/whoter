using iTextSharp.text;
using System;

namespace water3
{
    internal class Document
    {
        private Rectangle rectangle;
        private int v1;
        private int v2;
        private int v3;
        private int v4;

        public Document(Rectangle rectangle, int v1, int v2, int v3, int v4)
        {
            this.rectangle = rectangle;
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.v4 = v4;
        }

        internal void Add(Paragraph title)
        {
            throw new NotImplementedException();
        }

        internal void Open()
        {
            throw new NotImplementedException();
        }
    }
}