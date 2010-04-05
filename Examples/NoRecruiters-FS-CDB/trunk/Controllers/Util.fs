namespace NoRecruiters
    module Util =
        let normalize v =
            if System.String.IsNullOrEmpty v then System.String.Empty
            else v.Trim()

        let safeLower (s: string) =
            if s = null then s
            else s.ToLower()

        let charMap = Map.ofList
                        [
                        ('@', "at");
                        ('#', "sharp");
                        ('$', "dollar");
                        ('%', "percent");
                        ('*', "star");
                        ('+', "plus");
                        (':', "colon");
                        (',', "comma");
                        ('.', "dot");
                        (' ', "_");
                        ('(', System.String.Empty);
                        (')', System.String.Empty);
                        ('{', System.String.Empty);
                        ('}', System.String.Empty);
                        ('|', System.String.Empty);
                        ('[', System.String.Empty);
                        (']', System.String.Empty);
                        ('\\', System.String.Empty);
                        ('"', System.String.Empty);
                        (';', System.String.Empty);
                        ('\'', System.String.Empty);
                        ('<', System.String.Empty);
                        ('>', System.String.Empty);
                        ('?', System.String.Empty);
                        ('/', System.String.Empty);
                        ('`', System.String.Empty);
                        ('~', System.String.Empty);
                        ('!', System.String.Empty);
                        ('^', System.String.Empty);
                        ('&', System.String.Empty);
                        ]

        let sanitize s = 
            String.collect (fun c -> match Map.tryFind c charMap with | Some r -> r | None -> c.ToString()) s