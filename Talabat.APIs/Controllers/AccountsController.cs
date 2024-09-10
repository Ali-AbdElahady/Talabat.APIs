using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTO;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	public class AccountsController : APIBaseController
	{
		private readonly UserManager<AppUser> userManager;
		private readonly SignInManager<AppUser> signInManager;
		private readonly ITokenServices tokenServices;
		private readonly IMapper mapper;

		public AccountsController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager , ITokenServices tokenServices, IMapper mapper)
        {
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.tokenServices = tokenServices;
			this.mapper = mapper;
		}
        // Register
        [HttpPost("Register")]
		public async Task<ActionResult<UserDto>> Register(RegisterDto model)
		{
			if (CheckEmailExists(model.Email).Result.Value) { 
				return BadRequest(new ApiResponse(400,"This Email Is Already Exist.")); 
			}
			var user = new AppUser()
			{
				DisplayName = model.DisplayName,
				Email = model.Email,
				UserName = model.Email.Split('@')[0],
				PhoneNumber = model.PhoneNumber
			};
			var Result = await userManager.CreateAsync(user,model.Password);
			if (!Result.Succeeded) return BadRequest(new ApiResponse(400));
			return new UserDto()
			{
				DisplayName = model.DisplayName,
				Email = model.Email,
				Tokken = await tokenServices.CreateTokenAsync(user, userManager)
			};
		}

		// Login
		[HttpPost("Login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto model)
		{
			var user = await userManager.FindByEmailAsync(model.Email);
			if (user is null) return Unauthorized(new ApiResponse(401));
			var result = await signInManager.CheckPasswordSignInAsync(user, model.Password,false);
			if(!result.Succeeded) return Unauthorized(new ApiResponse(401));
			return Ok(new UserDto()
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				Tokken = await tokenServices.CreateTokenAsync(user,userManager)
			});
		}

		// Get Current User
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpGet("GetCurrentUser")]
		public async Task<ActionResult<UserDto>> GetCurrentUser()
		{
			var Email = User.FindFirstValue(ClaimTypes.Email);
			if (Email == null) return Unauthorized(new ApiResponse(401,"Unauthorized"));
			var user = await userManager.FindByEmailAsync(Email);
			if(user is null) return NotFound(new ApiResponse(404, "can not found this user"));
			return new UserDto()
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				Tokken = await tokenServices.CreateTokenAsync(user, userManager)
			};

		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpGet("Address")]
		public async Task<ActionResult<Address>> GetCurrentUserAddress()
		{
			//var Email = User.FindFirstValue(ClaimTypes.Email);
			//var user = userManager.FindByEmailAsync(Email);
			// Because of User have navigational property the privious way will not work
			// Then we will use the Extension method on userManager
			var user = await userManager.FindUserWithAddressAsync(User);
			var MappedAddress = mapper.Map<Address,AddressDto>(user.Address);
			return Ok(MappedAddress);
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpPut("Address")]
		public async Task<ActionResult<AddressDto>> UpdateAddress(AddressDto UpdatedAddress)
		{
			var user = await userManager.FindUserWithAddressAsync(User);
			if(user is null) return Unauthorized(new ApiResponse(401));
			var address = mapper.Map<AddressDto, Address>(UpdatedAddress);
			// beacuse of it create a new object with new id then it will remove the old address and add anthor one
			// so we create equal the old Id with The new Id
			address.Id = user.Address.Id;
			user.Address = address;
			var result = await userManager.UpdateAsync(user);
			if (!result.Succeeded) return BadRequest(new ApiResponse(401));
			return Ok(UpdatedAddress);
		}

		[HttpGet("eamilExists")]
		public async Task<ActionResult<bool>> CheckEmailExists(string email)
		{
			return await userManager.FindByEmailAsync(email) is not null;
		}
	}
}
