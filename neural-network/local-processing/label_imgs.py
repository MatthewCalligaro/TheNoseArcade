import cv2

import os

X = 0
Y = 0

# mouse callback function
def get_points(event,x,y,flags,param):
    global X
    global Y
    if event == cv2.EVENT_LBUTTONDOWN:
        print(x, y)
        X = x
        Y = y

def get_coors(img_fn):
    global X
    global Y

    # Open the image
    img = cv2.imread(img_fn)

    # Set up to read from the mouse
    cv2.namedWindow('image')
    cv2.setMouseCallback('image',get_points)

    # Display the image
    cv2.imshow('image',img)
    cv2.waitKey(0)

    # Write the result to a file
    f = open(img_fn[:-4] + '.txt', 'w')
    f.write(str(X) + '\n' + str(Y))
    f.close()

    # Destroy the window
    cv2.destroyAllWindows()

def draw_labels(img_fn):
    # Open the image
    img = cv2.imread(img_fn)

    # Set up to read from the mouse
    cv2.namedWindow('image')

    # Read in the point
    f = open(img_fn[:-4] + '.txt', 'r')
    x = int(f.readline())
    y = int(f.readline())
    f.close()

    # Add the point
    cv2.circle(img,(x,y), 3, (0,0,255), -1)

    # Display the image
    cv2.imshow('image',img)

    cv2.waitKey(0)

    # Destroy the window
    cv2.destroyAllWindows()

def process_dir(img_dir, show_comp):
    img_fns = os.listdir(img_dir)
    for fn in img_fns:
        if fn[-4:] == '.jpg':
            if show_comp:
                draw_labels(img_dir+fn)
            else:
                get_coors(img_dir+fn)

if __name__ == '__main__':
    # Set up paths
    top_dir = '/Users/nandeekanayak/Downloads/gt_db/'
    done_dirs = ['s05', 's02', 's34', 's33', 's32', 's35', 's04']

    # Check if the user wants to skip the above directories
    show_comp = input('Show completed?') == 'yes'

    img_dirs = os.listdir(top_dir)
    for dir in img_dirs:
        print(dir)
        if len(os.path.split(dir)[1]) == 3 and \
            ((show_comp and dir in done_dirs) or (not show_comp and dir not in done_dirs)):
            process_dir(top_dir + dir + '/', show_comp)
            cont = input("Continue?")
            if cont != 'yes':
                break
    