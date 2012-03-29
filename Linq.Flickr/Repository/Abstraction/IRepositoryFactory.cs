using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linq.Flickr.Repository.Abstraction
{
    public interface IRepositoryFactory
    {
        IAuthRepository CreateAuthRepository();

        ICommentRepository CreateCommentRepository();

        IPeopleRepository CreatePeopleRepository();

        ITagRepository CreateTagRepository();

        IPhotoRepository CreatePhotoRepository();
    }
}
