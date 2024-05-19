using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NOS.Engineering.Challenge.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json;

namespace NOS.Engineering.Challenge.Database;

public class FastDatabase<TOut, TIn> : IDatabase<TOut, TIn>
{
    private readonly IMapper<TOut?, TIn> _mapper;

    MyDbContext _dbContext = new MyDbContext();

    public FastDatabase(IMapper<TOut?, TIn> mapper)
    {
        _mapper = mapper;
    }

    public Task<TOut?> Create(TIn item)
    {
        var id = Guid.NewGuid();
        var createdItem = _mapper.Map(id, item);

        if(createdItem != null)
        {
            _dbContext.Add(createdItem);
            _dbContext.SaveChanges();
        }

        return Task.FromResult(createdItem);
    }

    public Task<TOut?> Read(Guid id)
    {
        var content = _dbContext.Contents.Find(id);
        //return Task.FromResult(_mapper.FromObject(content));

        var jsonString = JsonSerializer.Serialize(content);
        var jsonObject = JsonSerializer.Deserialize<TOut>(jsonString);

        return Task.FromResult(jsonObject);        
    }

    public Task<IEnumerable<TOut?>> ReadAll()
    {
        var contents = _dbContext.Contents.ToList();
        //return Task.FromResult(_mapper.FromList(contents));

        var jsonString = JsonSerializer.Serialize(contents);
        var jsonObject = JsonSerializer.Deserialize<IEnumerable<TOut?>>(jsonString);

        return Task.FromResult(jsonObject);
    }

    public Task<IEnumerable<TOut?>> SearchContents(String title, List<string> genres)
    {
        var contents = from c in _dbContext.Contents.ToList()
                       where (title == null || c.Title == title)
                      && (genres.Count == 0 || c.GenreList.Intersect(genres).Any())
                       select c;

        var jsonString = JsonSerializer.Serialize(contents);
        var jsonObject = JsonSerializer.Deserialize<IEnumerable<TOut?>>(jsonString);

        return Task.FromResult(jsonObject);
    }
    public Task<TOut?> Update(Guid id, TIn item)
    {
        var content = _dbContext.Contents.Find(id);
        if (content != null)
        {
            //var dbItem = _mapper.FromObject(content);
            var jsonString = JsonSerializer.Serialize(content);
            var dbItem = JsonSerializer.Deserialize<TOut>(jsonString);

            var updatedItem = _mapper.Patch(dbItem, item);
            Content updatedContent = (updatedItem as Content);

            content.Title = updatedContent.Title;
            content.SubTitle = updatedContent.SubTitle;
            content.Description = updatedContent.Description;
            content.ImageUrl = updatedContent.ImageUrl;
            content.Duration = updatedContent.Duration;
            content.StartTime = updatedContent.StartTime;
            content.EndTime = updatedContent.EndTime;
            content.GenreList = updatedContent.GenreList;

            _dbContext.SaveChanges();
            //return Task.FromResult(_mapper.FromObject(content));

            return Task.FromResult(updatedItem);
        }

        return Task.FromResult<TOut>(default!)!;
    }

    public Task<Guid> Delete(Guid id)
    {
        var content = _dbContext.Contents.Find(id);
        if(content != null)
        {
            _dbContext.Contents.Remove(content);
            _dbContext.SaveChanges();
        }

        return Task.FromResult(id);
    }

}