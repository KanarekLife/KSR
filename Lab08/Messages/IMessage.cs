namespace Messages;

public interface IMessage1
{
    public string? Text1 { get; set; }
}

public interface IMessage2
{
    public string? Text2 { get; set; }
}

public interface IMessage3 : IMessage1, IMessage2 { }
