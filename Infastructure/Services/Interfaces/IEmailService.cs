using Domain.Dtos;
using MimeKit.Text;

namespace Infrastructure.Services;

public interface IEmailService
{
    void SendEmail(MessageDto model,TextFormat format);
}