using System.Threading.Tasks;

namespace WTS_MVVMBinding.Activation
{
    public interface IActivationHandler
    {
        bool CanHandle(object args);

        Task HandleAsync(object args);
    }
}
