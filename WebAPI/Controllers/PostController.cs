using DomainLayer.Custom;
using DomainLayer.DTO;
using DomainLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceLayer.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Areas.Admin.ViewModels;

namespace WebApplication1.Areas.Admin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : Controller
    {
        private readonly IPostService _service;
        private readonly UserManager<AppUser> _userManager;
        public PostController(IPostService service, UserManager<AppUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }
        // GET: PostController
        [HttpGet]
        public IActionResult Index(PostVM vModel)
        {
            PostVM result;
            if (vModel.PageInfo == null)
            {
                vModel.PageInfo = new PageInfo()
                {
                    Controller="Post"
                };
            }
            result = vModel;
            vModel.PostList = _service.Index(vModel.PageInfo);
            vModel.PageSizeSelectList = new SelectList(vModel.PageInfo.PageSizeList);
            return Ok(result);
        }

        // GET: PostController/Details/5
        [HttpGet("{id:int}")]
        public IActionResult Details(int id)
        {
            
            PostDetailsVM vModel = new()
            {
                PostDetailsDTO = _service.Details(id) 
            };
            List<SelectListItem> list = new();
            foreach(var item in vModel.PostDetailsDTO.CategoryList)
            {
                list.Add(new SelectListItem()
                {
                    Text=item.CategorySelectDTO.Name
                });
            }
            vModel.CategorySelectList = list;
           
            return Ok(vModel);
        }

        // GET: PostController/Create
        public IActionResult Create()
        {
            
            PostCreateVM vModel = new ()
            {
                CategorySelectList = new SelectList(_service.CategorySelectList(), "Id", "Name").ToList()
            };
            
            return Ok(vModel);

        }

        // POST: PostController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(PostCreateVM vModel)
        {
            
            //try
            //{
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(this.User);
                    vModel.PostCreateDTO.UserId = user.Id;
                    _service.Create(vModel.PostCreateDTO);
                    return Ok("Create successfully");
                }
                else
                {
                    return BadRequest("Please add post");
                }
            //}
            //catch
            //{
            //    result = StatusCode(500, "Internal server error");
            //}
          
        }

        // GET: PostController/Edit/5
        public IActionResult Edit(int id)
        {
            
            PostEditVM vModel = new()
            {
                CategorySelectList = new MultiSelectList(_service.CategorySelectList(), "Id", "Name").ToList(),
                PostEditDTO = _service.Edit(id)
            };
            vModel.PostEditDTO.CategoryIdList = vModel.PostEditDTO.PostCategories.Select(x => x.CategoryId).ToList();
            
            return Ok(vModel);
        }

        // POST: PostController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PostEditVM vModel)
        {
           
            //try
            //{
                if (ModelState.IsValid)
                {
                    var isEdit=_service.Get(vModel.PostEditDTO.Id);
                    if (isEdit == null)
                    {
                        return NotFound("Course Not Found!!!!!!");
                    }
                    else
                    {
                        _service.Update(vModel.PostEditDTO);
                        return Ok(vModel);
                    }
                    
                }
                else
                {
                    vModel.CategorySelectList = new SelectList(_service.CategorySelectList(), "Id", "Name").ToList();
                    return BadRequest("Please Add Post");
                }
            //}
            //catch
            //{
            //    result = StatusCode(500, "Internal server error");
            //}
            //return result;
        }

        // GET: PostController/Delete/5
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return Ok("Post delete successfully");
        }

        // POST: PostController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(List<int> idList)
        {
          
            
                foreach (var item in idList)
                {
                    _service.Delete(item);
                }
                return Ok("Posts delete successfully");
           

        }
    }
}
