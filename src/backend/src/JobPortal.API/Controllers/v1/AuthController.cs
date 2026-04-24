using JobPortal.API.Models;
using JobPortal.Application.Features.Auth.Commands.ChangePassword;
using JobPortal.Application.Features.Auth.Commands.ForgotPassword;
using JobPortal.Application.Features.Auth.Commands.Login;
using JobPortal.Application.Features.Auth.Commands.Logout;
using JobPortal.Application.Features.Auth.Commands.RefreshToken;
using JobPortal.Application.Features.Auth.Commands.Register;
using JobPortal.Application.Features.Auth.Commands.ResetPassword;
using JobPortal.Application.Features.Auth.Commands.UpdateProfile;
using JobPortal.Application.Features.Auth.Commands.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>Register a new job seeker or employer account.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Created(
            string.Empty,
            ApiResponse<RegisterResponse>.Ok(result, "Registration successful."));
    }

    /// <summary>Authenticate and receive JWT access + refresh tokens.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        // Enrich command with request metadata
        var enriched = command with
        {
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            DeviceInfo = Request.Headers.UserAgent.ToString()
        };

        var result = await mediator.Send(enriched, cancellationToken);

        // Refresh token stored in HttpOnly cookie for XSS protection
        Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        // Return response without refresh token in body
        var response = result with { RefreshToken = string.Empty };
        return Ok(ApiResponse<LoginResponse>.Ok(response, "Login successful."));
    }

    /// <summary>Refresh the access token using the HttpOnly refresh token cookie.</summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RefreshTokenCommand(), cancellationToken);
        return Ok(ApiResponse<RefreshTokenResponse>.Ok(result, "Token refreshed."));
    }

    /// <summary>Change the authenticated user's password.</summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        // Clear refresh cookie — user must re-login on other devices
        Response.Cookies.Delete("refreshToken");
        return Ok(ApiResponse.Ok("Password changed successfully."));
    }

    /// <summary>Invalidate the current session's refresh token.</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await mediator.Send(new LogoutCommand(), cancellationToken);
        Response.Cookies.Delete("refreshToken");
        return Ok(ApiResponse.Ok("Logged out successfully."));
    }

    /// <summary>Send a password reset email if the account exists.</summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        // Always return 200 to prevent email enumeration
        return Ok(ApiResponse.Ok("If an account exists for that email, a reset link has been sent."));
    }

    /// <summary>Reset password using a valid time-limited token.</summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok(ApiResponse.Ok("Password reset successfully. Please log in with your new password."));
    }

    /// <summary>Verify email address using signed token from the verification email.</summary>
    [HttpPost("verify-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyEmail(
        [FromBody] VerifyEmailCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok(ApiResponse.Ok("Email verified successfully."));
    }

    /// <summary>Resend the verification email to the given address.</summary>
    [HttpPost("resend-verification")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ResendVerification(
        [FromBody] ResendVerificationRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new ResendVerificationCommand(request.Email), cancellationToken);
        return Ok(ApiResponse.Ok("If an unverified account exists for that email, a new verification link has been sent."));
    }

    /// <summary>Update the authenticated user's profile (name, gender, DOB, avatar).</summary>
    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok(ApiResponse.Ok("Profile updated successfully."));
    }
}

public sealed record ResendVerificationRequest(string Email);
