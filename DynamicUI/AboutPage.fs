namespace DynamicUI

module AboutPage =

    type Msg = ContactDetails


    type Model =
        { Name: string
          Email: string
          Twitter: string
          Github: string
          LinkedIn: string }

    let init =
        { Name = "Edgar Gonzalez"
          Email = "edgargonzalez.info@gmail.com"
          Twitter = ""
          Github = ""
          LinkedIn = "" }

    let update msg model =
        match msg with
        | ContactDetails -> model

    let view model _ =

        let aboutEntries =
            StackLayout.stackLayout
                [ StackLayout.Children
                    [ Label.label
                        [ Label.Text model.Name
                          Label.Margin 16.0 ]
                      Label.label
                          [ Label.Text model.Email
                            Label.Margin 16.0 ]
                      Label.label
                          [ Label.Text model.Twitter
                            Label.Margin 16.0 ]
                      Label.label
                          [ Label.Text model.Github
                            Label.Margin 16.0 ]
                      Label.label
                          [ Label.Text model.LinkedIn
                            Label.Margin 16.0 ] ] ]

        let content =
            ScrollView.scrollView [ ScrollView.Content aboutEntries ]

        ContentPage.contentPage
            [ ContentPage.Title "About me"
              ContentPage.Content content ]
