namespace water3.Models
{
    public class ComboItem
    {
        public string Text { get; }
        public string Value { get; }

        public ComboItem(string text, string value)
        {
            Text = text;
            Value = value;
        }

        public override string ToString() => Text;
    }
}
