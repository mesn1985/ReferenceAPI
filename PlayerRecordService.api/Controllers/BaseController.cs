using Microsoft.AspNetCore.Mvc;

namespace SkycavePlayerService.api.Controllers
{
    /// <summary>
    /// Base controller for the controller implementations
    /// </summary>
    /// <typeparam name="T">Should be the type of the base class</typeparam>
    [ApiController]
    [Produces("application/json")]
    public abstract class BaseController<T> : ControllerBase
    {
        protected readonly ILogger<T> logger;

        protected BaseController(ILogger<T> logger)
        {
            this.logger = logger;
        }
    }
}
