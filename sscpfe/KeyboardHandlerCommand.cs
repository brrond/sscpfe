namespace sscpfe
{
    /* TODO: clear this file after
     * undo:
     * 
     * Backspace        => Place cursor in position after and insert(del_char)
     * Del              => Place cursor in position (before == after) and insert(del_char) 
     * 
     * Enter            => Place cursor in postion after and delete()
     * Insert (Default) => Place cursor in position after and delete()
     * 
     * CtrlBackspace    => Place cursor in position after and insert(del_charS)
     * CtrlDel          => Place cursor in position (before == after) and insert(del_charS)
     * 
     * Tab              => Place cursor in position after and delete() * 4
     * CtrlV            => Place cursor in position after and ...
    */
    enum KeyboardHandlerCommand
    {
        UpArrow,
        DownArrow,
        LeftArrow,
        RightArrow,
        Backspace,
        Enter,
        Home,
        End,
        Esc,
        CtrlV,
        CtrlBackspace,
        Tab,
        CtrlDel,
        Del,
        CtrlLeftArrow,
        CtrlRightArrow,
        CtrlZ,
        CtrlY,
        Default
    }
}
