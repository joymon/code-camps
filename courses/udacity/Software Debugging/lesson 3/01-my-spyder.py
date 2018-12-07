import sys
def remove_html_markup(s):
    #print("called")
    tag = False
    quote = False
    out = ""
    for c in s:
        #assert not tag
        if c == '<' and not quote:
            tag = True
            #print("stripped")
        elif c == '>' and not quote:
            tag = False
        elif c == '"' or c == "'" and tag:
            quote = not quote
        elif not tag:
            out = out + c
    return out

inp = "<f>'PhillyDotNet-CodeCamp2016-ModernWeb'</f></f>"
out = remove_html_markup(inp)
#print(out)

stepping = True
breakpoints = {9: True, 14: True}

def traceit (frame, event, arg):
    global stepping
    global breakpoints
    #print(breakpoints.get(frame.f_lineno,False))
    #print(event, breakpoints.get(9,False))
    if event == 'line':
        #if frame.f_lineno == 9:
        if stepping and breakpoints.get(frame.f_lineno,False):
            print(frame.f_code.co_name, frame.f_lineno, frame.f_locals)
    return traceit

sys.settrace(traceit)
print(remove_html_markup(inp))
sys.settrace(None)