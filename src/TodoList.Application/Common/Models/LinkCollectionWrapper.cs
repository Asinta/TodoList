namespace TodoList.Application.Common.Models;

public class LinkCollectionWrapper<T> : LinkResourceBase
{
    public List<T> Value { get; set; } = new ();
    
    public LinkCollectionWrapper()
    {
    }

    public LinkCollectionWrapper(List<T> value) => Value = value;
    
}