namespace LambdaUI.Models.Bot
{
    internal class ConfigModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public override string ToString() => $"Key: '{Key}' | Value '{Value}'";
    }
}