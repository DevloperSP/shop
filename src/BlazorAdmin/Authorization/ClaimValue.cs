namespace BlazorAdmin.Authorization
{
    public class ClaimValue
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public ClaimValue() { }

        public ClaimValue(string type, string value)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
