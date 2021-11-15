using ImageGallery.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace ImageGallery.Data
{
    public class ImageGalleryDbContext : IdentityDbContext
    {
        
        public ImageGalleryDbContext(DbContextOptions options) : base(options)
        {


        }

        public DbSet<GalleryImage> GalleyImages { get; set; }
        public DbSet<ImageTag> ImageTags { get; set; }
    }

    
}

