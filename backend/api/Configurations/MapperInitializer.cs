using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Entities;
using AutoMapper;

namespace api.Configurations
{
    public class MapperInitializer : Profile
    {
        public MapperInitializer()
        {
            CreateMap<TodoItem, TodoDto>().ReverseMap();
            CreateMap<TodoItem, TodoCreateDto>().ReverseMap();
        }
    }
}