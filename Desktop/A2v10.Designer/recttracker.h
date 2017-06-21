#pragma once


#define CX_HANDLE_SIZE 6 

#define RTRE_TOPLEFT     0x00000001
#define RTRE_TOPRIGHT    0x00000002
#define RTRE_BOTTOMRIGHT 0x00000004
#define RTRE_BOTTOMLEFT  0x00000008
#define RTRE_TOP         0x00000010
#define RTRE_RIGHT       0x00000020
#define RTRE_BOTTOM      0x00000040
#define RTRE_LEFT        0x00000080
#define RTRE_MIDDLE      0x00000100
#define RTRE_ALL         0x000001FF

class CRectTrackerEx : public CRectTracker
{
public:
	CRectTrackerEx(LPCRECT lpSrcRect, UINT nStyle, bool bPartial = false);
	CRectTrackerEx(bool bPartial = false);
	void DrawEx(CDC* pDC, bool bOutline);
	void DrawItem(CDC* pDC, bool bOutline);
	BOOL SetCursorEx(CWnd* pWnd, UINT nHitTest) const;
	virtual UINT GetHandleMask() const;
	DWORD GetDrawMask(int hit) const { return DWORD(1L << hit); }
	BOOL CanHitHandled(int hit) { return m_dwDrawStyle & GetDrawMask(hit); }

protected:
	bool m_bPartial;
	static CBrush m_brHatch;

	CBrush* GetHatchBrush();

	void DrawHatchBorder(CDC* pDC, CRect& rect);

public:
	DWORD m_dwDrawStyle;
};
