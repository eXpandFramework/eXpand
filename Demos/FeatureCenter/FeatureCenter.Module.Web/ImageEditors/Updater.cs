﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace FeatureCenter.Module.Web.ImageEditors {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            if (ObjectSpace.FindObject<PictureObject>(null) == null) {
                var masterObject = ObjectSpace.CreateObject<PictureMasterObject>();
                masterObject.Title = "masterobject";
                var pictureObjects = GetAlbums();
                if (pictureObjects.Any()){
                    masterObject.HorizontalPicObjects.AddRange(pictureObjects);
                    masterObject.HorizontalPicObjects[0].ImagePath = "";
                    masterObject.HorizontalPicObjectsStyleModified.AddRange(pictureObjects);
                    masterObject.HorizontalPicObjectsStyleModified[0].ImagePath = null;
                    masterObject.VerticalPicObjects.AddRange(pictureObjects);
                    masterObject.VerticalPicObjectsStyleModified.AddRange(pictureObjects);
                    masterObject.HorizontalPicObjectsWithNoImage.AddRange(pictureObjects);
                    masterObject.VerticalPicObjectsWithNoImage.AddRange(pictureObjects);
                }
                ObjectSpace.CommitChanges();
            }
        }

        IEnumerable<PictureObject> GetAlbums(int count = 20) {
            try{
                var random = new Random();
                var albums = new List<PictureObject>();
                for (int i = 0; i < count; i++) {
                    Bitmap bitmap = GetBitmap(random);
                    string mapPath = HttpContext.Current.Server.MapPath("/");
                    string filename = Path.Combine(mapPath, string.Format("Title Title{0}.jpg", i));
                    bitmap.Save(filename, ImageFormat.Jpeg);
                    string absoluteUri =
                        HttpContext.Current.Request.Url.AbsoluteUri.Replace(
                            HttpContext.Current.Request.Url.PathAndQuery, "/");
                    albums.Add(GetPictureObject(i, absoluteUri));
                }
                return albums;
            }
            catch {
                return Enumerable.Empty<PictureObject>(); 
            }
        }

        PictureObject GetPictureObject(int i, string absoluteUri) {
            var pictureObject = ObjectSpace.CreateObject<PictureObject>();
            pictureObject.ImagePath = absoluteUri + string.Format("Title Title{0}.jpg", i);
            pictureObject.Title = "Title Title" + i;
            pictureObject.SubTitle = "SubTitle SubTitle" + i;
            return pictureObject;
        }

        Bitmap GetBitmap(Random random) {
            var bitmap = new Bitmap(75, 75);
            for (int j = 0; j < 75; j++) {
                for (int k = 0; k < 75; k++) {
                    bitmap.SetPixel(j, k,
                                    Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)));
                }
            }
            return bitmap;
        }
    }
}