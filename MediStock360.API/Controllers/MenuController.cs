using System.Collections.Generic;
using System.Threading.Tasks;
using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediStock360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        #region User Navigation Menus

        /// <summary>
        /// Retrieves the personalized, dynamic navigation menu tree for the logged-in user,
        /// filtered based on active client, store, and assigned roles/permissions.
        /// </summary>
        [HttpGet("nav")]
        public async Task<IActionResult> GetNavMenus()
        {
            var result = await _menuService.GetNavMenusForCurrentUserAsync();
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves dynamic menu hierarchy accessible to a specific role.
        /// </summary>
        [HttpGet("role/{roleId:int}")]
        public async Task<IActionResult> GetMenusByRoleId(int roleId)
        {
            var result = await _menuService.GetAccessibleMenusForRoleAsync(roleId);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        #endregion

        #region Menu Master Management (Admin)

        /// <summary>
        /// Retrieves all system menus organized in a nested parent-child hierarchy tree.
        /// </summary>
        [HttpGet("tree")]
        public async Task<IActionResult> GetAllMenusTree()
        {
            var result = await _menuService.GetAllMenusTreeAsync();
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a flat list of all system menus.
        /// </summary>
        [HttpGet("flat")]
        public async Task<IActionResult> GetAllMenusFlat()
        {
            var result = await _menuService.GetAllMenusFlatAsync();
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a single menu item by its identifier.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMenuById(int id)
        {
            var result = await _menuService.GetMenuByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Creates a new top-level menu or child submenu with optional permission code.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _menuService.CreateMenuAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing menu or submenu.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] UpdateMenuRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _menuService.UpdateMenuAsync(id, dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Toggles active/inactive status of a menu.
        /// </summary>
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> ToggleMenuStatus(int id)
        {
            var result = await _menuService.ToggleMenuStatusAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Deletes a menu item and any associated child submenus.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var result = await _menuService.DeleteMenuAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Seeds default system menus and submenus for MediStock360 modules.
        /// </summary>
        [HttpPost("seed-default")]
        public async Task<IActionResult> SeedDefaultMenus()
        {
            var result = await _menuService.SeedDefaultMenusAsync();
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        #endregion
    }
}

