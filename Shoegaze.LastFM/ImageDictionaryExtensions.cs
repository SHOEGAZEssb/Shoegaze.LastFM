namespace Shoegaze.LastFM
{
	/// <summary>
	/// Extensions for <see cref="IReadOnlyDictionary{TKey, TValue}"/> image uri dictionaries.
	/// </summary>
	public static class ImageDictionaryExtensions
	{
		/// <summary>
		/// Get the uri of the largest available image size.
		/// Null, if dictionary does not contain any uris.
		/// </summary>
		/// <param name="images">Dictionary to get uri from.</param>
		/// <returns>Uri of the largest available image size, or null.</returns>
		public static Uri? GetLargestOrDefault(this IReadOnlyDictionary<ImageSize, Uri> images)
		{
			if (images.TryGetValue(ImageSize.Mega, out Uri? result))
				return result;
			else if (images.TryGetValue(ImageSize.ExtraLarge, out result))
				return result;
			else if (images.TryGetValue(ImageSize.Large, out result))
				return result;
			else if (images.TryGetValue(ImageSize.Medium, out result))
				return result;
			else if (images.TryGetValue(ImageSize.Small, out result))
				return result;
			else if (images.TryGetValue(ImageSize.Unknown, out result))
				return result;

			return null;
		}
	}
}
