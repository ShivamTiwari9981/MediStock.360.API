using System.Threading.Tasks;
using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediStock360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        #region Permission Master Endpoints

        /// <summary>
        /// Retrieves all permissions, with optional filtering by module name and active status.
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPermissions([FromQuery] string? moduleName = null, [FromQuery] bool? isActive = null)
        {
            var result = await _permissionService.GetAllPermissionsAsync(moduleName, isActive);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves active permissions grouped by module.
        /// </summary>
        [HttpGet("grouped")]
        public async Task<IActionResult> GetPermissionsGroupedByModule()
        {
            var result = await _permissionService.GetPermissionsGroupedByModuleAsync();
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a single permission by ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            var result = await _permissionService.GetPermissionByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Creates a new system permission.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _permissionService.CreatePermissionAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing permission.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] UpdatePermissionRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _permissionService.UpdatePermissionAsync(id, dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Toggles the active status of a permission.
        /// </summary>
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> TogglePermissionStatus(int id)
        {
            var result = await _permissionService.TogglePermissionStatusAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Deletes a permission and cleans up its mappings.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var result = await _permissionService.DeletePermissionAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        #endregion

        #region Role-Permission Mapping Endpoints

        /// <summary>
        /// Retrieves permissions for a specific role along with an assignment flag.
        /// </summary>
        [HttpGet("role/{roleId:int}")]
        public async Task<IActionResult> GetPermissionsByRoleId(int roleId)
        {
            var result = await _permissionService.GetPermissionsByRoleIdAsync(roleId);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Assigns / maps a list of permissions to a specific role.
        /// </summary>
        [HttpPost("role/assign")]
        public async Task<IActionResult> AssignPermissionsToRole([FromBody] AssignRolePermissionsRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _permissionService.AssignPermissionsToRoleAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        #endregion

        #region User-Role Mapping Endpoints (Multi-Role Support)

        /// <summary>
        /// Retrieves available roles for a user along with an assignment flag.
        /// </summary>
        [HttpGet("user/{userId:long}/roles")]
        public async Task<IActionResult> GetRolesByUserId(long userId)
        {
            var result = await _permissionService.GetRolesByUserIdAsync(userId);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Assigns / maps multiple roles to a user.
        /// </summary>
        [HttpPost("user/assign-roles")]
        public async Task<IActionResult> AssignRolesToUser([FromBody] AssignUserRolesRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _permissionService.AssignRolesToUserAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);

        }

        /// <summary>
        /// Retrieves all effective permissions for a user aggregated across all assigned roles.
        /// </summary>
        [HttpGet("user/{userId:long}/permissions")]
        public async Task<IActionResult> GetUserPermissions(long userId)
        {
            var result = await _permissionService.GetUserPermissionsAsync(userId);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        #endregion
    }
}

