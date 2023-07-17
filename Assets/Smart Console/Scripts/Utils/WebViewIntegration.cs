using UnityEngine;

public class WebViewIntegration : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        WebViewIntegration.InitializeWebView();
    }

    private static void InitializeWebView()
    {
        string webViewCode =
            @"
            <script>
                // Disable the context menu to prevent the default browser download behavior
                document.addEventListener('contextmenu', function (e) {
                    e.preventDefault();
                });

                // Add an event listener for handling the pop-up download
                window.addEventListener('click', function () {
                    var link = document.createElement('a');
                    link.style.display = 'none';
                    document.body.appendChild(link);

                    link.addEventListener('click', function (e) {
                        e.stopPropagation();
                        document.body.removeChild(link);
                    });
                });
            </script>
        ";

        Application.ExternalEval(webViewCode);
    }
}
