using System.Threading.Tasks;

namespace main.Services.Interfaces
{
    public interface IPublisherService
    {
        Task PublishAsync(string queueName, string message);
    }
}