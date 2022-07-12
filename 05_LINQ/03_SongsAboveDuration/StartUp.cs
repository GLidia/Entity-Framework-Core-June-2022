namespace MusicHub
{
    using System;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
            Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Albums.Where(x => x.ProducerId == producerId)
                .ToList()
                .Select(x => new
            {
                Name = x.Name,
                ReleaseDate = x.ReleaseDate.Date.ToString("MM/dd/yyyy"),
                ProducerName = x.Producer.Name,
                Songs = x.Songs.Select(y => new
                {
                    SongName = y.Name,
                    SongPrice = y.Price,
                    SongWriterName = y.Writer.Name
                }).OrderByDescending(y => y.SongName).ThenBy(y => y.SongWriterName).ToList(),
                Price = x.Price
            }).OrderByDescending(x => x.Price).ToList();

            var sb = new StringBuilder();

            foreach(var album in albums)
            {
                int songCount = 0;
                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine("-Songs:");
                foreach(var song in album.Songs)
                {
                    songCount++;
                    sb.AppendLine($"---#{songCount}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.SongPrice:F2}");
                    sb.AppendLine($"---Writer: {song.SongWriterName}");
                }
                sb.AppendLine($"-AlbumPrice: {album.Price:F2}");
            }

            return sb.ToString().Trim();            
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs
                .ToList()
                .Where(x => x.Duration.TotalSeconds > duration)
                .Select(x => new
                {
                    Name = x.Name,
                    PerformerFullName = x.SongPerformers.Select(y => $"{y.Performer.FirstName} {y.Performer.LastName}").FirstOrDefault(),
                    WriterName = x.Writer.Name,
                    AlbumProducer = x.Album.Producer.Name,
                    Duration = x.Duration.ToString("c")
                }).OrderBy(x => x.Name).ThenBy(x => x.WriterName).ThenBy(x => x.PerformerFullName).ToList();

            var sb = new StringBuilder();

            int songCount = 0;

            foreach(var song in songs)
            {
                songCount++;
                sb.AppendLine($"-Song #{songCount}");
                sb.AppendLine($"---SongName: {song.Name}");
                sb.AppendLine($"---Writer: {song.WriterName}");
                sb.AppendLine($"---Performer: {song.PerformerFullName}");
                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                sb.AppendLine($"---Duration: {song.Duration}");
            }

            return sb.ToString().Trim();
        }
    }
}
