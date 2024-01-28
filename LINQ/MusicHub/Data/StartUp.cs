using MusicHub.Data;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MusicHub
{
  public class StartUp
    {
        static void Main(string[] args)
        {
            MusicHubDbContext context =
                   new MusicHubDbContext();

            Console.WriteLine(ExportSongsAboveDuration(context, 120));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var sb = new StringBuilder();

            var albums = context.Albums
                .Where(a => a.ProducerId == producerId)
                .Select(a => new
                {
                    Name = a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs.OrderByDescending(s => s.Name).ThenBy(s => s.Writer).ToList(),
                    Price = a.Price,
                    TotalPrice = a.Songs.Sum(p => p.Price)
                })
                .ToList();

            foreach (var album in albums.OrderByDescending(a => a.TotalPrice))
            {
                int count = 1;

                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine("-Songs:");

                foreach (var song in album.Songs)
                {
                    sb.AppendLine($"---#{count}");
                    sb.AppendLine($"---SongName: {song.Name}");
                    sb.AppendLine($"---Price: {song.Price:f2}");
                    sb.AppendLine($"---Writer: {song.Writer.Name}");

                    count++;
                }

                sb.AppendLine($"-AlbumPrice: {album.TotalPrice:f2}");
            }

            return sb.ToString().Trim();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var sb = new StringBuilder();

            int count = 1;

            var songs = context.Songs
                .AsEnumerable()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    Name = s.Name,
                    Performer = s.SongPerformers
                        .Select(s => $"{s.Performer.FirstName} {s.Performer.LastName}")
                        .OrderBy(p => p)
                        .ToArray(),
                    Writer = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                })
                .ToList()
                .OrderBy(s => s.Name)
                .ThenBy(s => s.Writer);

            foreach (var song in songs)
            {
                sb.AppendLine($"-Song #{count}");
                sb.AppendLine($"---SongName: {song.Name}");
                sb.AppendLine($"---Writer: {song.Writer}");
                if (song.Performer.Any())
                {
                    foreach (var performer in song.Performer)
                    {
                        sb.AppendLine($"---Performer: {performer}");
                    }
                }
                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                sb.AppendLine($"---Duration: {song.Duration}");

                count++;
            }

            return sb.ToString().Trim();
        }
    }
}