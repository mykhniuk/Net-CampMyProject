using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Net_CampMyProject.Data.Models;

namespace FilmsParser.Extensions
{
    public class ApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public ApiClient(string apiBaseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiBaseUrl)
            };
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task<Person> AddOrGetPersonAsync(Person person)
        {
            var allPersons = await _httpClient.GetAsync<List<Person>>(Endpoints.Persons);
            var existingPerson = allPersons.FirstOrDefault(p => p.Name == person.Name);

            return existingPerson ?? await _httpClient.PostAsJsonAsyncEx(Endpoints.Persons, person);
        }

        public async Task<Film> GetFilmByTitleOrDefaultAsync(string title)
        {
            var getAllFilms = await _httpClient.GetAsync<List<Film>>(Endpoints.Films);
            return getAllFilms.FirstOrDefault(f => f.Title == title);
        }

        public async Task<Film> AddFilmAsync(Film film)
        {
            return await _httpClient.PostAsJsonAsyncEx(Endpoints.Films, film);
        }

        public async Task<Genre> AddOrGetGenreAsync(string name)
        {
            var allGenres = await _httpClient.GetAsync<List<Genre>>(Endpoints.Genres);

            var existingGenre = allGenres.FirstOrDefault(g => g.GenreType == name);
            if (existingGenre != null)
                return existingGenre;

            return await _httpClient.PostAsJsonAsyncEx(Endpoints.Genres, new Genre {GenreType = name});
        }

        public async Task AddFilmGenreAsync(int newFilmId, int genreId)
        {
            var newFilmGenre = new FilmGenre
            {
                FilmId = newFilmId,
                GenreId = genreId
            };

            await _httpClient.PostAsJsonAsyncEx(Endpoints.FilmGenres, newFilmGenre);
        }

        public async Task<FilmPerson> AddFilmPerson(int newFilmId, int newPersonId, FilmPersonRole role)
        {
            return await _httpClient.PostAsJsonAsyncEx(Endpoints.FilmPersons, new FilmPerson
            {
                FilmId = newFilmId,
                PersonId = newPersonId,
                Role = role
            });
        }

        public static class Endpoints
        {
            public static string FilmRatingSources = "FilmRatingSources";
            public static string FilmPersons = "FilmPersons";
            public static string FilmRatings = "FilmRatings";
            public static string Films = "Films";
            public static string Persons = "Persons";
            public static string FilmGenres = "FilmGenres";
            public static string Genres = "Genres";
        }
        public async Task<FilmRatingSource> AddOrGetRatingSource(string resourceWebsite)
        {
            var allRatingSources = await _httpClient.GetAsync<List<FilmRatingSource>>(Endpoints.FilmRatingSources);

            var existingGenre = allRatingSources.FirstOrDefault(g => g.ResourceWebsite == resourceWebsite);
            if (existingGenre != null)
                return existingGenre;

            return await _httpClient.PostAsJsonAsyncEx(Endpoints.FilmRatingSources, new FilmRatingSource() { ResourceWebsite = resourceWebsite });
        }
        public async Task<FilmRating> AddFilmRating(int newFilmId, int newRatingSourceId, string rating)
        {
            return await _httpClient.PostAsJsonAsyncEx(Endpoints.FilmRatings, new FilmRating()
            {
                FilmId = newFilmId,
                SourceId= newRatingSourceId,
                Rating = rating

            });
        }


    }
}