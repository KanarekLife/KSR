using Messages;

namespace Publisher;

record Message1 : IMessage1
{
    public string? Text1 { get; set; }
}

record Message2 : IMessage2
{
    public string? Text2 { get; set; }
}

record Message3 : IMessage3
{
    public string? Text1 { get; set; }
    public string? Text2 { get; set; }
}
