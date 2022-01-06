namespace TodoList.Application.Common.Models;

public class LinkResourceBase
{
    public LinkResourceBase()
    {
    }

    public List<Link> Links { get; set; } = new ();
}