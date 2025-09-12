using Microsoft.AspNetCore.Mvc;

namespace ErrorHandlingDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ErrorHandlingController : ControllerBase
    {
        private readonly ILogger<ErrorHandlingController> _logger;

        public ErrorHandlingController(ILogger<ErrorHandlingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("divide")]
        public IActionResult GetDivisionResult(int numerator, int denominator)
        {
            try
            {
                int result = numerator / denominator;
                _logger.LogInformation("Division succeeded: {Numerator} / {Denominator} = {Result}",
                    numerator, denominator, result);

                return Ok(new { Result = result });
            }
            catch (DivideByZeroException)
            {
                _logger.LogError("Division by zero is not allowed. Numerator: {Numerator}", numerator);
                return BadRequest(new { Error = "Division by zero is not allowed." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during division.");
                return StatusCode(500, new { Error = "An unexpected server error occurred." });
            }
        }
    }
}
