using System.Web;
using System.Web.Optimization;

namespace MvcEasyui
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/jquery_min").Include(
                        "~/Scripts/jquery-1.10.2.min.js"));

            bundles.Add(new ScriptBundle("~/jquery-val").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/easyui").Include("~/Scripts/jquery.easyui.min.js", "~/Scripts/easyui-lang-zh_CN.js"));
            bundles.Add(new StyleBundle("~/easyui/css").Include(
                      "~/Content/themes/bootstrap/easyui.css", "~/Content/themes/icon.css"));
        }
    }
}
