using System.Threading.Tasks;

namespace WTS_MVVMBinding.Contracts.Services
{
    public interface IActivationService
    {
        Task ActivateAsync(object activationArgs);
    }
}
