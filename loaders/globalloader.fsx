#r "../_lib/Fornax.Core.dll"

type SiteInfo = {
    title: string
    description: string
    postPageSize: int
}

let loader (projectRoot: string) (siteContent: SiteContents) =
    let siteInfo =
        { title = "Michal's ramblings";
          description = ""
          postPageSize = 5 }
    siteContent.Add(siteInfo)

    siteContent
