using System.Collections;
using CommonTestUtils.Requests;

namespace UseCases.Test.InlineDatas;

public class ImageTypesInlineData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var images = FormFileBuilder.ImageCollection();
        foreach (var img in images)
            yield return [img];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}