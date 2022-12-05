#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html
open Layout


let generate' (ctx : SiteContents) (post: Postloader.Post) =
    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo> ()
    let desc =
        siteInfo
        |> Option.map (fun si -> si.description)
        |> Option.defaultValue ""

    Layout.layout ctx post.title [
        section [Class "hero is-primary is-medium is-bold"] [
            div [Class "hero-body"] [
                div [Class "container has-text-centered"] [
                    h1 [Class "title"] [!!desc]
                ]
            ]
        ]
        div [Class "container"] [
            section [Class "articles"] [
                div [Class "column is-8 is-offset-2"] [
                    Layout.postLayout false post
                ]
            ]
        ]
    ]

let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
    ctx.TryGetValues<Postloader.Post> ()
    |> Option.defaultValue Seq.empty
    |> Seq.tryFind (fun n -> n.file = page)
    |> Option.map (generate' ctx)
    |> Option.map (Layout.render ctx)
    |> Option.defaultValue ""