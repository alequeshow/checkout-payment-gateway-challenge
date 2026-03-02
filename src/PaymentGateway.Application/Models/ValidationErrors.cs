namespace PaymentGateway.Application.Models;

public class ValidationErrors : Dictionary<string, string[]>
{
    public void AddFieldError(string field, string message)
    {
        if(this.ContainsKey(field))
        {
            var errors = this.GetValueOrDefault(field);

            if(errors is not null)
            {
                this[field] = [.. errors, message];

                return;
            }
        }

        this.TryAdd(field, [ message ]);
    }
}
