using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Application.DTOs.Common;
using Application.DTOs.User;
using Application.Interfaces;
using Core.Interfaces;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly IFileService _fileService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, IAuthService authService, IFileService fileService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _authService = authService;
        _fileService = fileService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's profile
    /// </summary>
    [HttpGet("profile")]
    public async Task<ActionResult<ResponseDto<UserProfileDto>>> GetProfile()
    {
        var userId = GetCurrentUserId();
        var profile = await _userService.GetProfileAsync(userId);

        if (profile == null)
            return NotFound(new ResponseDto<UserProfileDto>
            {
                Success = false,
                Message = "User not found"
            });

        return Ok(new ResponseDto<UserProfileDto>
        {
            Success = true,
            Message = "Profile retrieved successfully",
            Data = profile
        });
    }

    /// <summary>
    /// Update current user's profile
    /// </summary>
    [HttpPut("profile")]
    public async Task<ActionResult<ResponseDto<UserProfileDto>>> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var profile = await _userService.UpdateProfileAsync(userId, dto);

            return Ok(new ResponseDto<UserProfileDto>
            {
                Success = true,
                Message = "Profile updated successfully",
                Data = profile
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update profile for user");
            return BadRequest(new ResponseDto<UserProfileDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Delete current user's account
    /// </summary>
    [HttpDelete("profile")]
    public async Task<ActionResult<ResponseDto<object>>> DeleteProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _userService.DeleteAccountAsync(userId);

            return Ok(new ResponseDto<object>
            {
                Success = true,
                Message = "Account deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete account for user");
            return BadRequest(new ResponseDto<object>
            {
                Success = false,
                Message = "Failed to delete account. Please try again."
            });
        }
    }

    /// <summary>
    /// Change current user's password
    /// </summary>
    [HttpPut("change-password")]
    public async Task<ActionResult<ResponseDto<object>>> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _authService.ChangePasswordAsync(userId, dto.NewPassword);

            return Ok(new ResponseDto<object>
            {
                Success = true,
                Message = "Password changed successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to change password for user");
            return BadRequest(new ResponseDto<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Upload profile picture
    /// </summary>
    [HttpPost("profile-picture")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ResponseDto<string>>> UploadProfilePicture(IFormFile file)
    {
        try
        {
            var userId = GetCurrentUserId();
            var url = await _fileService.UploadProfilePictureAsync(userId, file);

            return Ok(new ResponseDto<string>
            {
                Success = true,
                Message = "Profile picture uploaded successfully",
                Data = url
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload profile picture");
            return BadRequest(new ResponseDto<string>
            {
                Success = false,
                Message = "Failed to upload profile picture. Please try again."
            });
        }
    }

    /// <summary>
    /// Delete profile picture
    /// </summary>
    [HttpDelete("profile-picture")]
    public async Task<ActionResult<ResponseDto<object>>> DeleteProfilePicture()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _fileService.DeleteProfilePictureAsync(userId);

            return Ok(new ResponseDto<object>
            {
                Success = true,
                Message = "Profile picture deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete profile picture");
            return BadRequest(new ResponseDto<object>
            {
                Success = false,
                Message = "Failed to delete profile picture. Please try again."
            });
        }
    }

    /// <summary>
    /// Get profile picture URL
    /// </summary>
    [HttpGet("profile-picture")]
    public async Task<ActionResult<ResponseDto<string>>> GetProfilePicture()
    {
        var userId = GetCurrentUserId();
        var url = await _fileService.GetProfilePictureAsync(userId);

        return Ok(new ResponseDto<string>
        {
            Success = true,
            Message = url != null ? "Profile picture found" : "No profile picture",
            Data = url
        });
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid or missing user identity claim");
        return userId;
    }
}
