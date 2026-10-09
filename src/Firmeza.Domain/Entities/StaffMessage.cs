using Firmeza.Domain.Rules;

namespace Firmeza.Domain.Entities;

public sealed class StaffMessage
{
    private StaffMessage()
    {
        FromEmail = "";
        ToEmail = "";
        Text = "";
    }

    public Guid Id { get; private set; }

    public string FromEmail { get; private set; }

    public string ToEmail { get; private set; }

    public string Text { get; private set; }

    public DateTime Date { get; private set; }

    public static StaffMessage Create(string? deCorreo, string? paraCorreo, string? text, DateTime date)
    {
        var cuerpo = text?.Trim() ?? "";
        if (cuerpo.Length == 0)
        {
            throw new BusinessRuleException("El mensaje es obligatorio.");
        }

        if (cuerpo.Length > 500)
        {
            throw new BusinessRuleException("El mensaje no puede pasar de 500 caracteres.");
        }

        var de = Validation.Email(deCorreo);
        var para = Validation.Email(paraCorreo);
        if (de == para)
        {
            throw new BusinessRuleException("El mensaje necesita otro destinatario.");
        }

        return new StaffMessage
        {
            Id = Guid.NewGuid(),
            FromEmail = de,
            ToEmail = para,
            Text = cuerpo,
            Date = date,
        };
    }
}
