namespace Linq.Flickr
{
    public interface IQueryFactory
    {
        TagCollection CreateTagQuery();

        PeopleCollection CreatePeopleQuery();

        PhotoCollection CreatePhotoQuery();
    }
}