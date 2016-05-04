namespace MS.EntityAggregate.Dtos
{
    public enum EntityCommandType
    {
        Save,
        Delete
    }

    public class EntityCommand
    {
        public EntityCommandType Type { get; set; }
        public Entity Entity { get; set; }
    }
}