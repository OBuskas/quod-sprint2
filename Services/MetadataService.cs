using System.Text.RegularExpressions;
using ValidationAPI.Models;
using ExifLib;

namespace ValidationAPI.Services
{
    public class MetadataService
    {
        private readonly IConfiguration _configuration;

        public MetadataService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<UploadMetadata> ExtractMetadataAsync(IFormFile file)
        {
            try
            {
                // Validate file size
                var maxSize = _configuration.GetValue<int>("MaxFileSizeMB", 5) * 1024 * 1024;
                if (file.Length > maxSize)
                {
                    throw new InvalidOperationException($"File size exceeds maximum allowed size of {maxSize / 1024 / 1024} MB");
                }

                // Validate file type
                var allowedTypes = _configuration.GetValue<string[]>("AllowedFileTypes", new[] { "image/jpeg", "image/png" });
                if (!allowedTypes.Contains(file.ContentType))
                {
                    throw new InvalidOperationException($"File type {file.ContentType} is not allowed. Allowed types: {string.Join(", ", allowedTypes)}");
                }

                // Sanitize filename
                string fileName = Path.GetFileName(file.FileName);
                fileName = Regex.Replace(fileName, @"[^a-zA-Z0-9_.-]", "_");

                // Initialize metadata
                var metadata = new UploadMetadata
                {
                    FileName = fileName,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    CaptureDate = DateTime.UtcNow
                };

                // Extract EXIF data if available
                if (file.ContentType.StartsWith("image/"))
                {
                    using (var stream = file.OpenReadStream())
                    {
                        try
                        {
                            var reader = new ExifReader(stream);
                            
                            // Extract device information
                            if (reader.GetTagValue(ExifTags.Make, out string make))
                                metadata.Manufacturer = make;
                            
                            if (reader.GetTagValue(ExifTags.Model, out string model))
                                metadata.Model = model;

                            // Validate device manufacturer
                            var allowedManufacturers = _configuration.GetValue<string[]>("AllowedDeviceManufacturers", new[] { "Apple", "Samsung", "Google" });
                            if (metadata.Manufacturer != null && !allowedManufacturers.Contains(metadata.Manufacturer, StringComparer.OrdinalIgnoreCase))
                            {
                                throw new InvalidOperationException($"Device manufacturer {metadata.Manufacturer} is not allowed. Allowed manufacturers: {string.Join(", ", allowedManufacturers)}");
                            }

                            // Extract GPS location if available
                            if (reader.GetTagValue(ExifTags.GPSLatitude, out double latitude) &&
                                reader.GetTagValue(ExifTags.GPSLongitude, out double longitude))
                            {
                                // Get reference values
                                var latRef = reader.GetTagValue(ExifTags.GPSLatitudeRef, out string latRefStr) ? latRefStr : "N";
                                var lonRef = reader.GetTagValue(ExifTags.GPSLongitudeRef, out string lonRefStr) ? lonRefStr : "E";
                                
                                // Adjust for hemisphere
                                if (latRef == "S") latitude = -latitude;
                                if (lonRef == "W") longitude = -longitude;
                                
                                metadata.GpsLocation = $"{latitude},{longitude}";
                                
                                // Validate location if required
                                var allowedCountries = _configuration.GetValue<string[]>("AllowedCountries", new[] { "BR" });
                                if (allowedCountries?.Length > 0)
                                {
                                    // In a real implementation, you would use a geolocation service to validate the country
                                    // This is just a placeholder for the validation logic
                                    if (!IsLocationInAllowedCountries(latitude, longitude, allowedCountries))
                                    {
                                        throw new InvalidOperationException($"Location is not within allowed countries: {string.Join(", ", allowedCountries)}");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // If EXIF extraction fails, log the error and continue without metadata
                            Console.WriteLine($"Error extracting EXIF data: {ex.Message}");
                        }
                    }
                }

                return metadata;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to process file: {ex.Message}", ex);
            }
        }

        private bool IsLocationInAllowedCountries(double latitude, double longitude, string[] allowedCountries)
        {
            // This is a placeholder for actual geolocation validation
            // In a real implementation, you would:
            // 1. Use a geolocation service to get the country for the coordinates
            // 2. Compare it against the allowed countries
            // 3. Return true if the location is in an allowed country
            
            // For now, we'll just return true to allow all locations
            return true;
        }
    }
}
