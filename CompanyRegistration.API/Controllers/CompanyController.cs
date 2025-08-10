using CompanyRegistration.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace CompanyRegistration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        // signup
        [HttpPost("signup")]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        public async Task<Results<Ok<APIResult<CompanyResponseDto>>, BadRequest<APIResult<CompanyResponseDto>>>> SignUp(CompanySignUpRequestDto requestDto)
        {
            var result = await _companyService.SignUpAsync(requestDto);
            return result.Success
                ?TypedResults.Ok(result)
                :TypedResults.BadRequest(result);
        }

        // verfy otp 
        [HttpPost("verify-otp")]
        public async Task<Results<Ok<APIResult<string>>, BadRequest<APIResult<string>>>>
           VerifyOtp(VerifyOtpRequestDto requestDto)
        {
            var result = await _companyService.VerifyOtpAsync(requestDto);
            return result.Success
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }


        // set password
        [HttpPost("set-password")]
        public async Task<Results<Ok<APIResult<string>>, BadRequest<APIResult<string>>>>
            SetPassword(SetPasswordRequestDto requestDto)
        {
            var result = await _companyService.SetPasswordAsync(requestDto);
            return result.Success
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }

        // login
        [HttpPost("login")]
        public async Task<Results<Ok<APIResult<CompanyResponseDto>>, UnauthorizedHttpResult>> Login(LoginRequestDto requestDto)
        {
            var result = await _companyService.LoginAsync(requestDto);
            if (result.Success && result.Data != null)
            {
                return TypedResults.Ok(result);
            }

            return TypedResults.Unauthorized();
        }

        // get company by id
        [HttpGet("{id:int}")]
        //[Authorize]
        public async Task<Results<Ok<APIResult<CompanyResponseDto>>, NotFound<APIResult<CompanyResponseDto>>>>
            GetCompany(int id)
        {
            var result = await _companyService.GetCompanyByIdAsync(id);
            return result.Success
                ? TypedResults.Ok(result)
                : TypedResults.NotFound(result);
        }


    }
}
