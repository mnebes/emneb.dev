#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html

let about =
    [ "Hi, my name is Michal and this is my blog."
      """I'm a software engineer living in Zurich, Switzerland. I'm using this space to occasionally document something what I learned."""
      "I hope you enjoy whatever I put here!" ]

let generate' (ctx : SiteContents) (_: string) =
    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo> ()
    let desc =
        siteInfo
        |> Option.map (fun si -> si.description)
        |> Option.defaultValue ""


    Layout.layout ctx "About" [
        section [Class "hero is-info is-medium is-bold"] [
            div [Class "hero-body"] [
                div [Class "container has-text-centered"] [
                    h1 [Class "title"] [!!desc]
                ]
            ]
        ]
        div [Class "container"] [
            section [Class "articles"] [
                div [Class "column is-8 is-offset-2"] [
                    div [Class "card article"] [
                        div [Class "card-content"] [
                            div [Class "content article-body"] [
                                for paragraph in about do
                                    p [] [!! paragraph]
                                p [] [
                                    !! "You can find me on "
                                    !! """<a rel="me" href="https://mastodon.social/@emneb">Mastodon</a>"""
                                    span [Class "icon"] [
                                        img [ Src "/images/mastodon-l.svg"]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
      ]]

let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
    generate' ctx page
    |> Layout.render ctx
