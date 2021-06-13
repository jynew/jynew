import sys
import os
import codecs
import shutil
from chardet.universaldetector import UniversalDetector

import chardet    


def getAllFiles(path, file_list):
    dir_list = os.listdir(path)
    for x in dir_list:
        new_x = os.path.join(path, x)
        if os.path.isdir(new_x):
            getAllFiles(new_x, file_list)
        else:
            file_tuple = os.path.splitext(new_x)
            if file_tuple[1] == '.cs':
                file_list.append(new_x)
    return file_list


def get_encode_info(file):
    with open(file, 'rb') as f:
        data = f.read()
        return chardet.detect(data)['encoding']
    # return charenc
    # with open(file, 'rb') as f:
    #     detector = UniversalDetector()
    #     for line in f.readlines():
    #         detector.feed(line)
    #         if detector.done:
    #             break
    #     detector.close()
    #     return detector.result['encoding']


if __name__ == "__main__":
    files = []
    count = 0

    path = ''

    if len(sys.argv) > 1 and  os.path.exists(sys.argv[1]):
        path = sys.argv[1]
    else:
        path = "%s/../Assets/Scripts/" % os.path.dirname(os.path.realpath(sys.argv[0]))

    print('需要转换的路径为: ' + path)

    getAllFiles(path, files)

    for filename in files:
        encode_info = get_encode_info(filename)

        tempname = filename + "_temp"

        if encode_info != 'utf-8' and encode_info != 'ascii' and encode_info != None:
            BLOCKSIZE = 1048576  # or some other, desired size in bytes
            with codecs.open(filename, "r", encode_info) as sourceFile:
                print('file: [%s] 切换编码 [%s]-->[utf-8]' % (filename, encode_info))
                with codecs.open(tempname, "w", "utf-8") as targetFile:
                    while True:
                        contents = sourceFile.read(BLOCKSIZE)
                        if not contents:
                            break
                        targetFile.write(contents)

            shutil.move(tempname, filename)
            count += 1

    print('一共处理了: [%s]个文件' % count)
    input('Press any key to quit program.')