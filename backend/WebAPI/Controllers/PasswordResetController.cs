using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Auth;
using Application.DTOs.Common;
using Application.Interfaces;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/password-reset")]
public class PasswordResetController : ControllerBase
{
    private readonly IPasswordResetService _passwordResetService;
    private readonly ILogger<PasswordResetController> _logger;

    public PasswordResetController(IPasswordResetService passwordResetService, ILogger<PasswordResetController> logger)
    {
        _passwordResetService = passwordResetService;
        _logger = logger;
    }

    [HttpPost("request")]
    public async Task<ActionResult<ResponseDto<object>>> RequestPasswordReset([FromBody] RequestPasswordResetDto dto)
    {
        try
        {
            await _passwordResetService.RequestPasswordResetAsync(dto.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing password reset request");
        }

        // Always return success to prevent email enumeration
        return Ok(new ResponseDto<object>
        {
            Success = true,
            Message = "If an account with that email exists, a password reset link has been sent."
        });
    }

    [HttpPost("reset")]
    public async Task<ActionResult<ResponseDto<object>>> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        try
        {
            await _passwordResetService.ResetPasswordAsync(dto.Token, dto.NewPassword);

            return Ok(new ResponseDto<object>
            {
                Success = true,
                Message = "Password has been reset successfully."
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Password reset failed");
            return BadRequest(new ResponseDto<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpGet("validate/{token}")]
    public async Task<ActionResult<ResponseDto<bool>>> ValidateToken(string token)
    {
        var isValid = await _passwordResetService.ValidateTokenAsync(token);

        return Ok(new ResponseDto<bool>
        {
            Success = isValid,
            Message = isValid ? "Token is valid" : "Token is invalid or expired",
            Data = isValid
        });
    }
}
