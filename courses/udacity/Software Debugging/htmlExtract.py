def remove_html_markup(s):
    print("called")
    tag = False
    quote = False
    out = ""

    for c in s:
        #assert not tag
        if c == '<' and not quote:
            tag = True
            print("stripped")
        elif c == '>' and not quote:
            tag = False
        elif c == '"' or c == "'" and tag:
            quote = not quote
        elif not tag:
            out = out + c

    return out


inp = "<f>'PhillyDotNet-CodeCamp2016-ModernWeb'</f></f>"
out = remove_html_markup(inp)
print(out)

