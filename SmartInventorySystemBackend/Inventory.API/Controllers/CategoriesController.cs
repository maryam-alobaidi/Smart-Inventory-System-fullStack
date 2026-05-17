using Inventory.BusinessLogic;
using Inventory.DataAccess;
using Inventory.DTOs.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryModel>>> Get()
        {
            var Categories = await clsCategories.GetAllCategories();
            return Ok(Categories);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryModel>> GetById(int id)
        {
            var Category = await clsCategories.Find(id);
            if (Category == null)
            {
                return NotFound($"Category with {id} not found.");
            }
            return Ok(Category);
        }


        [HttpPost]
        public async Task<ActionResult<CategoryModel>> Add(CategoryModel category)
        {
            clsCategories newCategory = new clsCategories();
            newCategory.CategoryName = category.CategoryName;
          
            if (!await newCategory.Save())
            {
                return BadRequest("Failed to add new category.");
            }
            else
            {
                category.CategoryID = newCategory.CategoryID;
                return CreatedAtAction(nameof(GetById), new { id = category.CategoryID }, category);
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, CategoryModel categoryDTO)
        {
            // 1. نبحث عن البيانات
            var categoryFromDB = await clsCategoriesData.FindByID(id);
            if (categoryFromDB == null) return NotFound();

            // 2. نحول الـ DTO إلى كائن Business (هنا الفرق!)
            clsCategories businessCategory = new clsCategories();
            businessCategory.CategoryID = id;
            businessCategory.CategoryName = categoryDTO.CategoryName;
            businessCategory.Mode = clsCategories.enMode.update; // نخبره أنه تعديل

            // 3. الآن نستدعي Save من الكائن (Instance)
            if (await businessCategory.Save())
            {
                return NoContent();
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (await clsCategories.Delete(id))
            {
                return NoContent();
            }
            else
            {
                return BadRequest($"Failed to delete category with id {id}.");
            }
        }
    }
}

